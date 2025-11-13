// InputAdapter.js
class InputAdapter {
  constructor() {
    this.commands = {
      moveUp: false,
      moveDown: false, 
      moveLeft: false,
      moveRight: false,
      castPressed: false // Changed from 'cast' to 'castPressed'
    };
    
    this.keyMap = {
      'ArrowUp': 'moveUp',
      'ArrowDown': 'moveDown', 
      'ArrowLeft': 'moveLeft',
      'ArrowRight': 'moveRight',
      'w': 'moveUp',
      'W': 'moveUp',
      's': 'moveDown',
      'S': 'moveDown', 
      'a': 'moveLeft',
      'A': 'moveLeft',
      'd': 'moveRight', 
      'D': 'moveRight',
      ' ': 'castPressed'
    };

    console.log("🔄 InputAdapter created");
  }

  handleKeyDown(event) {
    console.log("⌨️ Key DOWN - key:", event.key, "code:", event.code);
    
    const command = this.keyMap[event.key];
    console.log("🔍 Mapped command:", command);
    
    if (command) {
      if (event.key === ' ') {
        event.preventDefault();
        console.log("🚫 Spacebar prevented default");
        
        // Only set castPressed if it's not already set (to prevent multiple triggers)
        if (!this.commands.castPressed) {
          this.commands.castPressed = true;
          console.log("✅ Cast PRESSED set to true");
        }
      } else {
        this.commands[command] = true;
        console.log("✅ Command set TRUE:", command);
      }
    } else {
      console.log("❌ No command mapped for key:", event.key);
    }
  } 

  handleKeyUp(event) {
    console.log("⌨️ Key UP - key:", event.key);
    
    const command = this.keyMap[event.key];
    if (command) {
      if (command === 'castPressed') {
        this.commands.castPressed = false;
        console.log("🔄 Cast PRESSED set to false");
      } else {
        this.commands[command] = false;
        console.log("🔄 Command set FALSE:", command);
      }
    }
  }

  getCommands() {
    // Create a snapshot that includes whether cast was pressed this frame
    const commands = { 
      ...this.commands,
      // We'll also include a one-time cast trigger that gets consumed
      castTrigger: this.commands.castPressed
    };
    
    console.log("📨 Getting commands:", commands);
    return commands;
  }

  // Method to clear the cast trigger after it's been used
  clearCastTrigger() {
    this.commands.castPressed = false;
  }

  reset() {
    this.commands = {
      moveUp: false,
      moveDown: false, 
      moveLeft: false,
      moveRight: false,
      castPressed: false
    };
  }
}

export default InputAdapter;