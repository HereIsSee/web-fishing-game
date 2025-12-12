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

  }

  handleKeyDown(event) {
    
    const command = this.keyMap[event.key];
    
    if (command) {
      if (event.key === ' ') {
        event.preventDefault();
        
        // Only set castPressed if it's not already set (to prevent multiple triggers)
        if (!this.commands.castPressed) {
          this.commands.castPressed = true;
        }
      } else {
        this.commands[command] = true;
      }
    } else {
    }
  } 

  handleKeyUp(event) {
    
    const command = this.keyMap[event.key];
    if (command) {
      if (command === 'castPressed') {
        this.commands.castPressed = false;
      } else {
        this.commands[command] = false;
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