FROM ubuntu:18.04

RUN apt -y update
RUN apt -y install wget apt-transport-https

# Install dotnet core runtime
# Taken from here: https://docs.microsoft.com/en-us/dotnet/core/install/linux-package-manager-ubuntu-1804
RUN wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN apt -y update
RUN apt -y install dotnet-runtime-3.1

# Install git, svn, and git-svn.
# Need git if we want to push
# Need SVN if we want to query for SVN branches.
RUN apt -y install git
RUN apt -y install subversion
RUN apt -y install git-svn

# Delete unneeded things:
RUN apt -y remove wget
RUN dpkg -r remove packages-microsoft-prod
RUN rm packages-microsoft-prod.deb
RUN apt -y clean
RUN apt -y autoclean

# Create user who does not have root access.
RUN useradd -d /svn2gitnetx -m svn2gitnetxuser 
RUN chown -R svn2gitnetxuser:svn2gitnetxuser /svn2gitnetx
USER svn2gitnetxuser

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
