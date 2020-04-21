FROM archlinux

# Update everything
RUN pacman -Syy --noconfirm
RUN pacman -Syu --noconfirm

# Install everything
RUN pacman -S dotnet-runtime --noconfirm
RUN pacman -S git --noconfirm
RUN pacman -S subversion --noconfirm
RUN pacman -S perl-term-readkey --noconfirm

# Clear cache now that everything is installed
RUN pacman -Sc --noconfirm

# Create user who does not have root access.
RUN useradd -d /svn2gitnetx -m svn2gitnetxuser 
RUN chown -R svn2gitnetxuser:svn2gitnetxuser /svn2gitnetx
USER svn2gitnetxuser

# Copy over everything
RUN mkdir /svn2gitnetx/repo/
RUN mkdir /svn2gitnetx/app/
COPY ./* /svn2gitnetx/app/

# Make our working directory the repo directory.
WORKDIR /svn2gitnetx/repo/

RUN svn --version
RUN git --version
RUN git svn --version

ENTRYPOINT [ "/svn2gitnetx/app/svn2gitnetx" ]
CMD [ "--help" ]
