# Svn2Git.NETX

**Svn2Git.NETX** Is a fork of [Svn2Git.NET](https://github.com/mazong1123/svn2gitnet).  The 'X' stands for eXtended.  These are extensions to Svn2Git.NET that I needed for my use cases.  It looks like the original Svn2Git.NET is no longer maintained, and the owner seems to have all but disappeared from GitHub, which is why this fork was created.

**Svn2Git.NETX** is re-written from [svn2git](https://github.com/nirvdrum/svn2git). It intends to provide a simple and easy way to migrate projects from svn to git.

**Svn2Git.NETX** is based on `git-svn` so please make sure `git-svn` has been installed.

## What is different from Svn2Git.NET?
* Hitting CTRL+C now kills all child processes instead of orphaning them.
* Can break SVN Locks via the "--breaklocks" argument on the command line.
* Uses dotnet core 3.1 instead of 2.0.

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

After `svn2gitnetx` is done with your project, you'll get this instead:

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

## How to use

### Initial Conversion

There are several ways you can create a git repo from an existing
svn repo. The differentiating factor is the svn repo layout. Below is an
enumerated listing of the varying supported layouts and the proper way to
create a git repo from a svn repo in the specified layout.

1. The svn repo is in the standard layout of (trunk, branches, tags) at the
root level of the repo.

```sh
svn2gitnetx http://svn.example.com/path/to/repo
```

2. The svn repo is NOT in standard layout and has only a trunk and tags at the
root level of the repo.

```sh
svn2gitnetx http://svn.example.com/path/to/repo --trunk dev --tags rel --nobranches
```

3. The svn repo is NOT in standard layout and has only a trunk at the root
level of the repo.

```sh
svn2gitnetx http://svn.example.com/path/to/repo --trunk trunk --nobranches --notags
```

4. The svn repo is NOT in standard layout and has no trunk, branches, or tags
at the root level of the repo. Instead the root level of the repo is
equivalent to the trunk and there are no tags or branches.

```sh
svn2gitnetx http://svn.example.com/path/to/repo --rootistrunk
```

5. The svn repo is in the standard layout but you want to exclude the massive
doc directory and the backup files you once accidently added.

```sh
svn2gitnetx http://svn.example.com/path/to/repo --exclude doc --exclude '.*~$'
```

6. The svn repo actually tracks several projects and you only want to migrate
one of them.

```sh
svn2gitnetx http://svn.example.com/path/to/repo/nested_project --no-minimize-url
```

7. The svn repo is password protected.

```sh
svn2gitnetx http://svn.example.com/path/to/repo --username <<user_with_perms>> --password <<password>>
```

8. You need to migrate starting at a specific svn revision number.

```sh
svn2gitnetx http://svn.example.com/path/to/repo --revision <<starting_revision_number>>
```

9. You need to migrate starting at a specific svn revision number, ending at a specific revision number.

```sh
 svn2gitnetx http://svn.example.com/path/to/repo --revision <<starting_revision_number>>:<<ending_revision_number>>
```

10. Include metadata (git-svn-id) in git logs.

```sh
svn2gitnetx http://svn.example.com/path/to/repo --metadata
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
svn2gitnetx --rebase
```

Optionally you can also use an alternate username/password instead of the default:

```sh
svn2gitnetx --rebase --username <<user>> --password <<password>>
```

# Authors

To convert all your svn authors to git format, create a file somewhere on your
system with the list of conversions to make, one per line, for example:

    mazong1123 = Jingyu Ma <mazong1123@gmail.com>
    xforever1313 = Seth Hendrick <seth@shendrick.net>

Then pass an `--authors` option to svn2gitnet pointing to your file:

```sh
svn2gitnetx http://svn.example.com/path/to/repo --authors ~/authors.txt
```

### Debugging

If you're having problems with converting your repository and you're not sure why,
try turning on verbose logging.  This will print out more information from the
underlying git-svn process as well as other trace information.

You can turn on verbose logging with the `-v` or `--verbose` flags, like so:

```sh
svn2gitnetx http://svn.yoursite.com/path/to/repo --verbose
```

### Options Reference

```sh
PS C:\> svn2gitnetx --help
svn2gitnetx 1.0.0
Copyright (C) 2020 Seth Hendrick, Jingyu Ma

  -v, --verbose        (Default: false) Be verbose in logging -- useful for debugging issues

  -m, --metadata       (Default: false) Include metadata in git logs (git-svn-id)

  --no-minimize-url    Accept URLs as-is without attempting to connect to a higher level directory

  --rootistrunk        Use this if the root level of the repo is equivalent to the trunk and there are no tags or
                       branches

  --trunk              (Default: trunk) Subpath to trunk from repository URL (default: trunk)

  --notrunk            Do not import anything from trunk

  --branches           Subpath to branches from repository URL (default: branches); can be used multiple times

  --nobranches         Do not try to import any branches

  --tags               Subpath to tags from repository URL (default: tags); can be used multiple times

  --notags             Do not try to import any tags

  --exclude            Specify a Perl regular expression to filter paths when fetching; can be used multiple times

  --revision           Start importing from SVN revision START_REV; optionally end at END_REV

  --username           Username for transports that needs it (http(s), svn)

  --password           Password for transports that need it (http(s), svn)

  --rebase             Instead of cloning a new project, rebase an existing one against SVN

  --rebasebranch       Rebase specified branch

  --authors            Path to file containing svn-to-git authors mapping

  --breaklocks         (Default: false) Breaks any index.lock files in the .git/svn/refs/remotes/svn/* directories.
                       Only use this if there are no processes running.

  --help               Display this help screen.

  --version            Display version information.
```

### Contribution

## Bug report & feature request
Bug report and feature request are always welcome. Please file an issue so that we can have a traceable discussion.

## Build and test the source code

### Prerequisite
- .NET Core Runtime 3.1.0 or newer. You can get the latest .NET Core Runtime from https://www.microsoft.com/net/core

- Make sure `git-svn` has been installed.

### Build

Svn2Git.NETX uses [Cake](https://cakebuild.net/) to build and run tests.  To install cake, simply install the cake dotnet core tool:

```sh
dotnet tool install -g Cake.Tool
```

Run following command to build the source code in the root of the repo:

Windows:

```sh
dotnet cake --target=build
``` 

### Run unit tests

If you only want to run unit tests other than full test run:

```sh
dotnet cake --target=unit_test
```

### Run integration tests

The integration tests require accessing external test svn repository and save the temp results in local folder. So currently some test cases related to private repository cannot be ran locally.

To run the integration tests:

Windows:

```sh
dotnet cake --target=integration_test
```
