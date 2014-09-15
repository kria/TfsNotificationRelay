# Tfs2Slack

Tfs2Slack is a plugin for Team Foundation Server 2013 that sends notifications to Slack.com.

## Features and TODO

- [x] Notify multiple Slack organizations and channels
- [x] Rule-based event filtering
- [x] Collection/project/repository regex filtering
- [x] Configurable notification format
- [x] Notification links to event in TFS web

### Events

- [x] Build completion
- [x] Work item update
- [x] Team project creation/deletion
- Git
  - [x] Push
  - [x] New branch/tag
  - [x] Deleted branch/tag
  - [x] Updated ref
  - [x] Lightweight/annotated tag
  - [x] Force-push
- TFVC
  - [x] Checkin
- [ ] Code reviews

## Screenshot

![Tfs2Slack screenshot](https://raw.githubusercontent.com/kria/Tfs2Slack/master/tfs2slack-notification.png)

## Installation

1. If you don't already have one, add an Incoming WebHooks integration under Configure Integrations in Slack.
2. Download the latest [release][0] or clone and build the source yourself.
3. Open `DevCore.Tfs2Slack.dll.config` and set at least your full unique Webhook URL in `slackWebhookUrl`. There are [more settings and formats][1] in there that should be somewhat self-explanatory.
4. Install the Tfs2Slack plugin by dropping `DevCore.Tfs2Slack.dll`, `DevCore.Tfs2Slack.dll.config` and `Newtonsoft.Json.dll` in *C:\Program Files\Microsoft Team Foundation Server 12.0\Application Tier\Web Services\bin\Plugins* on the server.

[0]: https://github.com/kria/Tfs2Slack/releases
[1]: https://github.com/kria/Tfs2Slack/blob/master/Tfs2Slack/app.config

## TFS 2013 version support

Because of a breaking API change in TFS 2013 Update 2, the plugin as is only works for TFS 2013 update 2 and 3 (and possibly later).
I will publish a branch with support for previous TFS 2013 versions at a later date.

## License

Copyright (C) 2014 Kristian Adrup

Tfs2Slack is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. See included file [COPYING](COPYING) for details.



