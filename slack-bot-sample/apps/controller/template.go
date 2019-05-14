package controller

import (
	"github.com/nlopes/slack"
)

// -> 알파고 템플릿 업데이트 token
func OnTemplateUpdate(sentUser string, token []string) (string, slack.PostMessageParameters) {
	// TODO 기능작업 해야 함
	params := slack.PostMessageParameters{}
	attachment := slack.Attachment{
		Color:      "good",
		Text:       "템플릿 데이터 업데이트",
		AuthorName: "Author - " + sentUser,

		Fields: []slack.AttachmentField{
			slack.AttachmentField{
				Title: "작업 파일",
				Value: token[2],
				Short: true,
			},
			slack.AttachmentField{
				Title: "실행 결과",
				Value: "성공",
				Short: true,
			},
		},
	}
	params.Attachments = []slack.Attachment{attachment}

	return "작업 수행 내역", params
}
