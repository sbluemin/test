package controller

import "github.com/nlopes/slack"

func OnYa(sentUser string, token []string) (string, slack.PostMessageParameters) {
	params := slack.PostMessageParameters{}
	return "뭐", params
}
