package config

import (
	"../../apps/controller"
	"github.com/nlopes/slack"
)

// 슬랙 봇 토큰 값
const APIToken = "xoxb-106450419042-5XkmQBA3DMuJ8CTkxzKU4r3p"

// 슬랙에서 사용되는 해당 봇의 이름
const BotSlackName = "alphago"

// 메시지 처리 맵
var MessageMap = map[string]func(sentUser string, token []string) (string, slack.PostMessageParameters){
	"템플릿업데이트": controller.OnTemplateUpdate,
	"야": controller.OnYa,
}
