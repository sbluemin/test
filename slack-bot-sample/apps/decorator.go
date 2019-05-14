package apps

import (
	"strings"

	"github.com/nlopes/slack"

	"../apps/config"
)

// 명령어를 인식하는 시작 값
const commandPrefix = "알파고"

// 알파고의 명령어 해석은 아래와 같이 진행 된다.
// 주어 + 목적어 + 동사 + value
// ex. 알파고 템플릿 업데이트 item_base
func RouteMessage(rtmSlack *slack.RTM, ev *slack.MessageEvent) {
	// 유저 정보를 가져온다
	userInfo, err := rtmSlack.GetUserInfo(ev.User)
	if err != nil {
		return
	}

	// 알파고 자신이 보낸 메시지는 무시
	if strings.Compare(userInfo.Name, config.BotSlackName) == 0 {
		return
	}

	// tokenize
	commands := strings.Split(ev.Text, " ")

	// 커맨드가 존재하지 않으면 리턴
	if len(commands) < 2 {
		return
	}

	// 명령어 Prefix 유효성 검사
	if strings.Compare(commands[0], commandPrefix) != 0 {
		return
	}

	// 메시지 처리 맵을 조사하여 처리를 넘김
	if function, isContain := config.MessageMap[commands[1]]; isContain {
		text, params := function(userInfo.Name, commands)
		params.AsUser = true
		rtmSlack.PostMessage(ev.Channel, text, params)
	} else {
		// 유효하지 않은 명령어일시 에러 메시지 출력
		params := slack.PostMessageParameters{}
		params.AsUser = true
		rtmSlack.PostMessage(ev.Channel, "유효하지 않은 명령어입니다.", params)
	}
}
