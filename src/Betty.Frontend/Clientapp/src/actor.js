import 'phaser';


export default class Actor {
    constructor(scene, left, top, name) {
        this.scene = scene;
        this.name = name;
        this.left = left;
        this.top = top;

        this.sprite = scene.add.sprite(left, top, name);

        this.sprite.setOrigin(0.5, 1);
    }

    say(text) {
        this.sprite.anims.play('talk', true);
    }

    stop() {
        this.sprite.anims.stop('talk', true);
    }
}