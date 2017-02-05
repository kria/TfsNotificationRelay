# TfsNotificationRelay

TfsNotificationRelay is an extensible plugin for Team Foundation Server that sends notifications to Slack, HipChat and IRC.

[![Build status](https://ci.appveyor.com/api/projects/status/f8tog2tftjbbotmr?svg=true)](https://ci.appveyor.com/project/kria/tfsnotificationrelay)

## Integrations

- Slack
- HipChat
- IRC
- SMTP
- Microsoft Teams

## Third-party Integrations

- [Skype for Business](https://github.com/thomasDOTde/TfsNotificationRelay)

## Features

- Notify multiple targets
- Rule-based event filtering
- Regex filtering on collection, project, repository, branch etc.
- Configurable notification format
- Notification links back to event in TFS web
- Extensible to support other targets

## Supported Events

- XAML Build completion
- vNext Build completion*
- Build quality change
- Work item update
- Team project creation/deletion
- Release creation*
- Release deployment*
- Git
  + Push and force-push
  + Pull request*
  + Comment on Pull request*
  + Comment on Commit*
  + Repository created
  + Repository renamed/deleted*
  + Branch created/deleted
  + Tag created/deleted (both lightweight and annotated)
  + Ref updated
- TFVC
  + Checkin
  + Comment on changeset*

*TFS 2015 only

## Screenshots

![Slack screenshot](https://raw.githubusercontent.com/kria/TfsNotificationRelay/master/slack-notifications.png)

![HipChat screenshot](https://raw.githubusercontent.com/kria/TfsNotificationRelay/master/hipchat-notifications.png)

## TFS version support

Since the TFS API changes quite frequently, there are multiple editions of TfsNotificationRelay. Make sure you pick the correct one for your system.

- TfsNotificationRelay for TFS 2013 (2013.2+)
- TfsNotificationRelay for TFS 2015 (2015.2+)
- TfsNotificationRelay for TFS 2017

Discontinued support (last builds available in [v1.16.0](https://github.com/kria/TfsNotificationRelay/releases/tag/v1.16.0)):

- TfsNotificationRelay for TFS 2015 RTM
- TfsNotificationRelay for TFS 2015.1

## Download

Download from [releases](https://github.com/kria/TfsNotificationRelay/releases).

## Installation & Configuration

See the [wiki](https://github.com/kria/TfsNotificationRelay/wiki)
 on how to install and configure TfsNotificationRelay.

## Building

Visual Studio 2015 is required since TfsNotificationRelay uses C# 6. All needed TFS dependecies are included, so you should be able to just clone and build.

## Branches

Branch     | Description                          | Status
-----------|--------------------------------------|-------
`master`   | TfsNotificationRelay for TFS 2017    | [![master status](https://ci.appveyor.com/api/projects/status/f8tog2tftjbbotmr/branch/master?svg=true)](https://ci.appveyor.com/project/kria/tfsnotificationrelay/branch/master)
`tfs2015`  | TfsNotificationRelay for TFS 2015.2+ | [![tfs2015 status](https://ci.appveyor.com/api/projects/status/f8tog2tftjbbotmr/branch/tfs2015?svg=true)](https://ci.appveyor.com/project/kria/tfsnotificationrelay/branch/tfs2015)
`tfs2013`  | TfsNotificationRelay for TFS 2013.2+ | [![tfs2013 status](https://ci.appveyor.com/api/projects/status/f8tog2tftjbbotmr/branch/tfs2013?svg=true)](https://ci.appveyor.com/project/kria/tfsnotificationrelay/branch/tfs2013)


## Extending TfsNotificationRelay

TfsNotificationRelay can easily be extended to send notifications to other services. Notifier modules referenced in the configuration file will be loaded dynamically at run time, so TfsNotificationRelay doesn't have to be recompiled.

1. Start a new class library project.
2. Add a reference to DevCore.TfsNotificationRelay.dll.
3. Create a class that implements the single method in `INotifier`. Take a look at the `SlackNotifier` class for pointers.
4. Build and drop in your new dll in the Plugins directory on the server.
5. Add a new bot element in `DevCore.TfsNotificationRelay.dll.config` with the correct assembly-qualified type name and settings.

## License

Copyright (C) 2014-2016 Kristian Adrup

TfsNotificationRelay is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. See included file [COPYING](COPYING) for details.
