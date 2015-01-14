# TfsNotificationRelay

TfsNotificationRelay is an extensible plugin for Team Foundation Server 2013 that sends notifications to Slack, HipChat and IRC.

[![Build status](https://ci.appveyor.com/api/projects/status/6jo9qqoqxjrgpwrp)](https://ci.appveyor.com/project/kria/tfs2slack)

## Features
- [x] Notify multiple targets
- [x] Rule-based event filtering
- [x] Collection/project/repository regex filtering
- [x] Configurable notification format
- [x] Notification links to event in TFS web
- [x] Extensible

### Events

- [x] Build completion
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

## Screenshot

![Slack screenshot](https://raw.githubusercontent.com/kria/TfsNotificationRelay/master/slack-notifications.png)

## Installation

### Slack

1. If you don't already have one, add an Incoming WebHooks integration under *Configure Integrations* in Slack.
2. Download the latest [release][0] or clone and build the source yourself.
3. Open `DevCore.TfsNotificationRelay.dll.config` and set at least your full unique Webhook URL in `slackWebhookUrl`. There are more [settings and formats][1] in there that should be somewhat self-explanatory.
4. Install the plugin by dropping `DevCore.TfsNotificationRelay.dll`, `DevCore.TfsNotificationRelay.Slack.dll`, `DevCore.TfsNotificationRelay.dll.config` and `Newtonsoft.Json.dll` in *C:\Program Files\Microsoft Team Foundation Server 12.0\Application Tier\Web Services\bin\Plugins* on the server.

[0]: https://github.com/kria/TfsNotificationRelay/releases
[1]: https://github.com/kria/TfsNotificationRelay/blob/master/TfsNotificationRelay/app.config

### HipCHat

### IRC

## Extending TfsNotificationRelay

## TFS 2013 version support

I'm referencing 2013.4 assemblies and testing primarily on a TFS 2013.4 server, so the plugin is most likely to work as intended on that version.
The development did start on TFS 2013.2 and the plugin probably still works on 2013.2 and 2013.3.
Because of a few breaking API changes in TFS 2013.2, the plugin won't work on previous versions without some minor modifications.
These are the API changes I know of in Update 2 that affect the plugin:
- Spelling fixed for PushNotification.AuthenticatedUserName (`Microsoft.TeamFoundation.Git.Server.dll`)
- WorkItemChangedEvent moved from `Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll` to `Microsoft.TeamFoundation.WorkItemTracking.Server.dll`

## License

Copyright (C) 2014-2015 Kristian Adrup

TfsNotificationRelay is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. See included file [COPYING](COPYING) for details.
