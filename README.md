```
     ██╗██████╗ ██████╗  ██████╗      ██████╗  █████╗ ███╗   ███╗███████╗
     ██║██╔══██╗██╔══██╗██╔════╝     ██╔════╝ ██╔══██╗████╗ ████║██╔════╝
     ██║██████╔╝██████╔╝██║  ███╗    ██║  ███╗███████║██╔████╔██║█████╗  
██   ██║██╔══██╗██╔═══╝ ██║   ██║    ██║   ██║██╔══██║██║╚██╔╝██║██╔══╝  
╚█████╔╝██║  ██║██║     ╚██████╔╝    ╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗
 ╚════╝ ╚═╝  ╚═╝╚═╝      ╚═════╝      ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝
```

# **JRPG-Game in C#**

## **Project Description**

This project is a console application developed in C# (.NET Core) simulating an RPG combat system. Players can build
their teams, select characters from different classes, and compete using various skills and tactical strategies.

The main objective is to provide an engaging gameplay experience while implementing the core principles of
object-oriented programming.

---

## **Features**

### **Characters**

#### Each character has:

- Name
- Team
- Health points (current and maximum)
- Physical and magical attack power
- Armor type: Textile, Leather, Mail, or Plates
- Dodge, Parade, and Spell resistance chances
- Speed
- Unique skills

#### Characters can:

- Use skills, attack or special ability
- Display detailed information

### **Classes**

- **Warrior :** *Durable and specialized in powerful physical attacks.*
- **Mage :** *Fragile but capable of devastating magical spells.*
- **Paladin :** *Versatile, mixing magic and physical attacks.*
- **Thief :** *Agile, with opportunistic attacks and evasive skills.*
- **Priest :** *Healer, supporting the team with healing spells and magic.*

### **Combat System**

- Turn management based on characters' speed
- Damage calculation considering armor type, dodge, parry, and spell resistance
- Detailed user interface for selecting actions and targets
- Simple victory condition: all enemy teams must be defeated

---

## **Instructions**

### **1. Installation**

1. Clone this repository:
   ```bash
   git clone https://github.com/erwnn20/JRPG-Game
   cd JRPG-Game
   ```
2. Make sure .NET Core is installed on your machine.
3. Build and run the project:
   ```bash
   dotnet run
   ```
   make sure you have installed .NET SDK 8.0.0

### **2. Usage**

1. Team Creation: Players take turns selecting characters for their teams.
2. Turn-Based Actions: Each player selects actions for their surviving characters.
3. End of Game: A team wins when all enemy characters are defeated.

---

## **Author**

Erwann
Varlet - [![GitHub](https://img.shields.io/badge/GitHub-181717?style=flat-square&logo=github)](https://github.com/erwnn20)  
Ynov Project 2024-2025
