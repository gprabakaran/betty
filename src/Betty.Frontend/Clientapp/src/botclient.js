import { DirectLine } from 'botframework-directlinejs';
import 'rxjs/add/operator/concatMap';

export default class BotClient {
    constructor(url, secret) {
        this.connector = new DirectLine({
            secret: secret,
            domain: url
        });
    }

    connect(messageHandler, eventHandler) {
        this.connector.activity$
            .filter(activity => activity.type === 'message' || activity.type === 'event')
            .concatMap(activity => {
                if(activity.type === 'message') {
                    return messageHandler(activity);
                }

                if(activity.type === 'event') {
                    return eventHandler(activity);
                }
            })
            .subscribe();
    }

    sendText(text) {
        this.connector.sendActivity({
            from: { id: this.conversationId },
            type: 'message',
            text: text
        });
    }

    sendEvent(type, data) {
        
    }
}