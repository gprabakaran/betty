import 'phaser';
import './index.css';

import Actor from './actor';
import ChatBox from './chatbox';
import BotClient from './botclient';

const config = {
    type: Phaser.AUTO,
    width: 800,
    height: 600,
    scene: {
        preload,
        create,
        update
    }
}

var game = new Phaser.Game(config);

function preload() {
    this.load.image('chatbox', 'assets/chatbox.png');
    this.load.image('desk', 'assets/desk.png');
    this.load.image('backdrop', 'assets/backdrop.png');

    this.load.spritesheet('bot', 'assets/bot.png', { frameWidth: 66, frameHeight: 147 }, 4);
}

function create() {
    this.anims.create({
        key: 'talk',
        frames: this.anims.generateFrameNumbers('bot', { start: 0, end: 3 }),
        frameRate: 10,
        repeat: -1
    });

    this.add.image(0,0, 'backdrop').setOrigin(0,0);
    
    var bot = new Actor(this, 550, 400, 'bot');
    var connector = new BotClient('http://localhost:20168/api/messages', '');

    this.add.image(500, 410, 'desk').setOrigin(0.5, 1);
    
    var chatbox = new ChatBox(this, (text) => {
        connector.sendText(text);
    });

    var botReply = this.add.text(50, 200, 'Bot reply', { font: '16px courier', 
        fill: '#ff0000', wordWrap: { width: 350, advancedWrap: true }})

    const eventHandler = (activity) => {
        botReply.text = activity.text;
    };

    const messageHandler = (activity) => {

    };

    connector.connect(messageHandler, eventHandler);
}

function update() {

}
