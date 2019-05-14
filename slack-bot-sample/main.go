package main

import (
	"fmt"
	"log"
	"os"

	"./apps"
	"./apps/config"

	"github.com/nlopes/slack"
)

func main() {
	// 로그를 담당하는 객체 초기화
	logger := log.New(os.Stdout, "slack-bot: ", log.Lshortfile|log.LstdFlags)
	slack.SetLogger(logger)

	// 전역 슬랙 API 객체
	apiSlack := slack.New(config.APIToken)

	// 전역 슬랙 RTM 객체
	rtmSlack := apiSlack.NewRTM()

	// check validation
	if apiSlack == nil || rtmSlack == nil {
		logger.Println("Initialize slack client error.")
		return
	}

	// active bot as user
	rtmSlack.SetUserAsActive()

	// Start real time message
	go rtmSlack.ManageConnection()

Loop:
	for {
		select {
		case msg := <-rtmSlack.IncomingEvents:
			switch ev := msg.Data.(type) {
			case *slack.ConnectedEvent:
				//RtmSlack.SendMessage(RtmSlack.NewOutgoingMessage("@sbluemin", "#general"))

			case *slack.MessageEvent:
				go apps.RouteMessage(rtmSlack, ev)

			case *slack.ChannelJoinedEvent:
				// 알파고가 새로운 채널에 입장했을 때 오는 콜백
				fmt.Println(ev.Channel.Name)

			case *slack.InvalidAuthEvent:
				break Loop
			}
		}
	}
}
