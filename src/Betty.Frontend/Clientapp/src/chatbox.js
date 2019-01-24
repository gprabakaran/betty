export default class ChatBox {
    constructor(scene, callback) {
        this.callback = callback;

        scene.add.image(5, 445, 'chatbox').setOrigin(0,0);
        
        var hintTextStyle = { 
            font: '8px Courier', 
            fill: '#fff' 
        };

        var hintText = scene.add.text(700, 580, 'Press ENTER to send.', hintTextStyle).setOrigin(1,1);

        var textEntryStyle = {
            font: '12px Courier',
            fill: '#fff',
            wordWrap: {
                width: 686,
                useAdvancedWrap: true
            }
        };

        var textEntry = scene.add.text(22, 457, '', textEntryStyle);

        // Bind the necessary keyboard callbacks to the text entry.
        // The provided user callback is invoked upon pressing ENTER.
        scene.input.keyboard.on('keydown', (event) => {
            // This junk is here to filter out any special keys. I might have missed a few, but that's fine.
            const hotkeys = [
                'F1','F2','F3','F4','F5','F6','F7','F8','F9','F10','F11','F12',
                'Backspace', 'Enter', 'Tab', 'ShiftRight','ShiftLeft', 'ControlRight','ControlLeft',
                'AltRight', 'AltLeft'
            ];

            if (event.keyCode === 8 && textEntry.text.length > 0) {
                textEntry.text = textEntry.text.substr(0, textEntry.text.length - 1);
            }
            else if (event.keyCode === 13) {
                // This signals the UI that we want to send a piece of text to the bot.
                // Clear the input after the callback is invoked so we can send another message.
                callback(textEntry.text);
                textEntry.text = '';
            } else if (hotkeys.indexOf(event.code) < 0) {
                textEntry.text += event.key;
            }

            console.log(event);
        });
    }
}