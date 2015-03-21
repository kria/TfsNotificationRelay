# TfsNotificationRelay

TfsNotificationRelay is an extensible plugin for Team Foundation Server 2013 that sends notifications to Slack, HipChat and IRC.

[![Build status](https://ci.appveyor.com/api/projects/status/f8tog2tftjbbotmr?svg=true)](https://ci.appveyor.com/project/kria/tfsnotificationrelay)

## Features
- [x] Notify multiple targets
- [x] Rule-based event filtering
- [x] Collection/project/repository regex filtering
- [x] Configurable notification format
- [x] Notification links to event in TFS web
- [x] Extensible

### Events

- [x] Build completion
- [x] Build quality change
- [x] Work item update
- [x] Team project creation/deletion
- Git
  - [x] Push
  - [x] Pull request
  - [x] New branch/tag
  - [x] Deleted branch/tag
  - [x] Updated ref
  - [x] Lightweight/annotated tag
  - [x] Force-push
- TFVC
  - [x] Checkin

## Screenshots

![Slack screenshot](https://raw.githubusercontent.com/kria/TfsNotificationRelay/master/slack-notifications.png)

![HipChat screenshot](https://raw.githubusercontent.com/kria/TfsNotificationRelay/master/hipchat-notifications.png)

## Installation

### Slack

1. If you don't already have one, add an Incoming WebHooks integration under *Configure Integrations* in Slack.
2. Download the latest [release][0] or clone and build the source yourself.
3. Open `DevCore.TfsNotificationRelay.dll.config` and set at least your full unique Webhook URL in `slackWebhookUrl`. There are more [settings and formats][1] in there that should be somewhat self-explanatory.
4. Install the plugin by dropping `DevCore.TfsNotificationRelay.dll`, `DevCore.TfsNotificationRelay.Slack.dll`, `DevCore.TfsNotificationRelay.dll.config` and `Newtonsoft.Json.dll` in *C:\Program Files\Microsoft Team Foundation Server 12.0\Application Tier\Web Services\bin\Plugins* on the server.

### HipChat

1. Create a room notification token for the HipChat room that you want to send notifications to.
2. Download the latest [release][0] or clone and build the source yourself.
3. Open `DevCore.TfsNotificationRelay.dll.config` and set at least `roomNotificationToken` and the corresponding `room` setting. There are more [settings and formats][1] in there that should be somewhat self-explanatory.
4. Install the plugin by dropping `DevCore.TfsNotificationRelay.dll`, `DevCore.TfsNotificationRelay.HipChat.dll`, `DevCore.TfsNotificationRelay.dll.config` and `Newtonsoft.Json.dll` in *C:\Program Files\Microsoft Team Foundation Server 12.0\Application Tier\Web Services\bin\Plugins* on the server.

[0]: https://github.com/kria/TfsNotificationRelay/releases
[1]: https://github.com/kria/TfsNotificationRelay/blob/master/TfsNotificationRelay/app.config

### IRC

See [TfsBot](https://github.com/kria/TfsBot).

## Configuration

See the [wiki](https://github.com/kria/TfsNotificationRelay/wiki) for more information.

## Extending TfsNotificationRelay

TfsNotificationRelay can easily be extended to send notifications to other services. Notifier modules referenced in the configuration file will be loaded dynamically at run time, so TfsNotificationRelay doesn't have to be recompiled.

1. Start a new class library project.
2. Add a reference to DevCore.TfsNotificationRelay.dll.
3. Create a class that implements the single method in `INotifier`. Take a look at the `SlackNotifier` class for pointers.
4. Build and drop in your new dll in the Plugins directory on the server.
5. Add a new bot element in `DevCore.TfsNotificationRelay.dll.config` with the correct assembly-qualified type name and settings.

## TFS 2013 version support

I'm referencing 2013.4 assemblies and testing primarily on a TFS 2013.4 server, so the plugin is most likely to work as intended on that version.
The development did start on TFS 2013.2 and the plugin probably still works on 2013.2 and 2013.3.
Because of a few breaking API changes in TFS 2013.2, the plugin won't work on previous versions without some minor modifications.
These are the API changes I know of in Update 2 that affect the plugin:
- Spelling fixed for PushNotification.AuthenticatedUserName in `Microsoft.TeamFoundation.Git.Server.dll`
- WorkItemChangedEvent moved from `Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll` to `Microsoft.TeamFoundation.WorkItemTracking.Server.dll`

## License

Copyright (C) 2014-2015 Kristian Adrup

TfsNotificationRelay is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. See included file [COPYING](COPYING) for details.
