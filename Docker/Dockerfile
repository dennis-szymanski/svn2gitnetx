FROM ubuntu:18.04

RUN apt -y update
RUN apt -y install wget apt-transport-https

# Install dotnet core runtime
# Taken from here: https://docs.microsoft.com/en-us/dotnet/core/install/linux-package-manager-ubuntu-1804
RUN wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN apt -y update
RUN apt -y install dotnet-runtime-3.1

# Install git-svn
RUN apt -y install git-svn

# Delete unneeded things:
RUN apt -y remove wget
RUN dpkg -r remove packages-microsoft-prod
RUN rm packages-microsoft-prod.deb
RUN apt -y clean
RUN apt -y autoclean

# Create user who does not have root access.
RUN useradd -d /svn2gitnet -m svn2gitnetuser 
RUN chown -R svn2gitnetuser:svn2gitnetuser /svn2gitnet
USER svn2gitnetuser

RUN mkdir /svn2gitnet/repo/
RUN mkdir /svn2gitnet/app/
COPY ./* /svn2gitnet/app/

# Make our working directory the repo directory.
WORKDIR /svn2gitnet/repo/

ENTRYPOINT [ "/svn2gitnet/app/svn2gitnet" ]
CMD [ "--help" ]
