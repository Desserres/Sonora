# 🎵 Sonora

Une application de lecteur de musique avec un serveur Node.js pour la gestion des médias et une base de données MySQL.





---

## 📁 Structure du projet

```
src/
├── Sonora/        # Application C#
├── NodeJSServer/       # Serveur Node.js (médias)
├── BackupBDD  # Backup de la base de données & Configuration Docker (MySQL + phpMyAdmin)
└── Server_Chat # Serveur pour le Chat TCP/IP
```

---

## 📷 Screenshot


### Login

<img width="1919" height="1029" alt="image" src="https://github.com/user-attachments/assets/edfaf2b5-0ea6-4dfd-a23d-eeb4d9991efb" />


### Creation De Compte

<img width="1919" height="1029" alt="image" src="https://github.com/user-attachments/assets/76c0d9a4-4d1a-461f-af3a-814ac0576493" />

### Sonora (Main App)

<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/d3abf1e1-e34b-4dd1-8a72-e41d710b4900" />


---

## 🖥️ MusicPlayer - Application C\#

Application principale du lecteur de musique, développée en C#.

---

## 🌐 NodeJSServer - Serveur d'application

Serveur Node.js permettant de récupérer et stocker les images ainsi que les fichiers `.mp3`.

### Prérequis

- [Node.js](https://nodejs.org/) installé

### Lancement

```bash
npm install
node server.js
```

---

## 🗄️ Base de données - MySQL via Docker

Le dossier contient le backup de la base de données (`MusicPlayer.sql`) ainsi que le fichier `docker-compose.yml` pour lancer les conteneurs MySQL et phpMyAdmin.

### 1. Démarrer les conteneurs

> ⚠️ Vérifier que la commande `dir` (ou `ls`) affiche bien le fichier `docker-compose.yml` avant de continuer.

```bash
docker compose up -d
```

Vérifier que les conteneurs sont bien créés :

```bash
docker ps -a
```

> Vous pouvez également les visualiser directement dans **Docker Desktop**.

### 2. Importer la base de données dans phpMyAdmin

1. Ouvrir l'interface phpMyAdmin (par défaut sur [http://localhost:8080](http://localhost:8080))
2. Créer une nouvelle base de données nommée **`MusicPlayer`**
3. Sélectionner la base `MusicPlayer`, puis aller dans l'onglet **Importer**
4. Importer le fichier **`MusicPlayer.sql`**

## 🐍 Script Python

`BackupBDD/addContentToDatabase.py`

Script Python permettant d’ajouter des musiques plus rapidement dans la base de données.

---

## 🚀 Démarrage rapide

```bash
# 1. Lancer la base de données
docker compose up -d

# 2. Démarrer le serveur Node.js
cd NodeJSServer
npm install
node server.js

# 3. Lancer le Serveur C#

# 4. Lancer l'application C#
# Ajouter le fichier .env et y définir la base URL du serveur d’application (BASE_URL=http://adresseIP:3000)
# Exécuter le projet
```
