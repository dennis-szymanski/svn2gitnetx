# SVN2GIT.NET

[![Build status](https://ci.appveyor.com/api/projects/status/g97ep0v2e9qbhemc?svg=true)](https://ci.appveyor.com/project/mazong1123/svn2gitnet)
[![Build status](https://travis-ci.org/mazong1123/svn2gitnet.svg?branch=master)](https://travis-ci.org/mazong1123/svn2gitnet)

**SVN2GIT.NET** started as a .NET re-implementation of [svn2git](https://github.com/nirvdrum/svn2git). It intends to provide a simple and easy way to migrate projects from svn to git.

It is based on `git-svn` so please make sure it has been installed.

## Examples

Say if we have this code in svn:

    trunk
      ...
    branches
      1.x
      2.x
    tags
      1.0.0
      1.0.1
      1.0.2
      1.1.0
      2.0.0

`git-svn` will go through the commit history to build a new git repo. It will
import all branches and tags as remote svn branches, whereas what you really
want is git-native local branches and git tag objects. So after importing this
project we'll get:

    $ git branch
    * master
    $ git branch -a
    * master
      1.x
      2.x
      tags/1.0.0
      tags/1.0.1
      tags/1.0.2
      tags/1.1.0
      tags/2.0.0
      trunk
    $ git tag -l
    [ empty ]

After `svn2gitnet` is done with your project, you'll get this instead:

    $ git branch
    * master
      1.x
      2.x
    $ git tag -l
      1.0.0
      1.0.1
      1.0.2
      1.1.0
      2.0.0

Finally, it makes sure the HEAD of master is the same as the current trunk of
the svn repo.

## Installation

### Windows

- Option 1: Download `.msi` file in the [release page](https://github.com/mazong1123/svn2gitnet/releases). Double click to install it. **Note: There's no "Next" or "Finish" button during the installation process. If it finished, just open a command line window, type `svn2gitnet --help` to verify the installation. I'll add wizard dialog during installation in the upcoming release.**

- Option: 2: Download `zip` file in the [release page](https://github.com/mazong1123/svn2gitnet/releases). Extract the zip, and add the folder to `PATH` environment variable. Open command line window and type `svn2gitnet --help`.

### Mac and *nix

Download the correct `tar.gz` file according to your OS. 

Extract it via:

```
tar -zxvf yourosname.tar.gz
```

Add the folder to the environment path:

```sh
PATH=$PATH:yourfolderpath
```

Type `svn2gitnet --help` to verify the installation.

## How to use

### Initial Conversion

There are several ways you can create a git repo from an existing
svn repo. The differentiating factor is the svn repo layout. Below is an
enumerated listing of the varying supported layouts and the proper way to
create a git repo from a svn repo in the specified layout.

1. The svn repo is in the standard layout of (trunk, branches, tags) at the
root level of the repo.

        $ svn2gitnet http://svn.example.com/path/to/repo

2. The svn repo is NOT in standard layout and has only a trunk and tags at the
root level of the repo.

        $ svn2gitnet http://svn.example.com/path/to/repo --trunk dev --tags rel --nobranches

3. The svn repo is NOT in standard layout and has only a trunk at the root
level of the repo.

        $ svn2gitnet http://svn.example.com/path/to/repo --trunk trunk --nobranches --notags

4. The svn repo is NOT in standard layout and has no trunk, branches, or tags
at the root level of the repo. Instead the root level of the repo is
equivalent to the trunk and there are no tags or branches.

        $ svn2gitnet http://svn.example.com/path/to/repo --rootistrunk

5. The svn repo is in the standard layout but you want to exclude the massive
doc directory and the backup files you once accidently added.

        $ svn2gitnet http://svn.example.com/path/to/repo --exclude doc --exclude '.*~$'

6. The svn repo actually tracks several projects and you only want to migrate
one of them.

        $ svn2gitnet http://svn.example.com/path/to/repo/nested_project --no-minimize-url

7. The svn repo is password protected.

        $ svn2gitnet http://svn.example.com/path/to/repo --username <<user_with_perms>> --password <<password>>

8. You need to migrate starting at a specific svn revision number.

        $ svn2gitnet http://svn.example.com/path/to/repo --revision <<starting_revision_number>>

9. You need to migrate starting at a specific svn revision number, ending at a specific revision number.

```sh
 svn2gitnet http://svn.example.com/path/to/repo --revision <<starting_revision_number>>:<<ending_revision_number>>
```

10. Include metadata (git-svn-id) in git logs.

```sh
svn2gitnet http://svn.example.com/path/to/repo --metadata
```

The above will create a git repository in the current directory with the git
version of the svn repository. Hence, you need to make a directory that you
want your new git repo to exist in, change into it and then run one of the
above commands. Note that in the above cases the trunk, branches, tags options
are simply folder names relative to the provided repo path. For example if you
specified trunk=foo branches=bar and tags=foobar it would be referencing
http://svn.example.com/path/to/repo/foo as your trunk, and so on. However, in
case 4 it references the root of the repo as trunk.

### Repository Updates

to pull in the latest changes from SVN into your
git repository created with svn2git.  This is a one way sync, but allows you to use svn2git
as a mirroring tool for your SVN repositories.

The command to call is:

```sh
svn2git --rebase
```