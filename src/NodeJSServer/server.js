const express = require('express');
const cors = require('cors');
const fs = require('fs');
const path = require('path');

const app = express();
app.use(cors());
app.use(express.json());

app.use('/images', express.static('public/images'));
app.use('/music', express.static('public/music'));

const dossierImages = "./public/images";
const dossierMusic = "./public/music";

const buildFileList = async () => {
    const files = [];

    try {
        const fichiers = await fs.promises.readdir(dossierMusic);

        const musiques = fichiers.filter(f => f.endsWith('.mp3'));

        for (let i = 0; i < musiques.length; i++) {
            const id = i + 1;
            const music = "http://127.0.0.1:3000/music/" + musiques[i];
            const image = "http://127.0.0.1:3000/images/" + musiques[i].replace('.mp3', '.jpg');
            files.push({ id, image, music });
        }

    } catch (err) {
        console.error(err);
    }

    return files;
};


const main = async () => {
  
    const delay = (ms) => (req, res, next) => setTimeout(next, ms);

    app.listen(3000, () => console.log('Server running at http://127.0.0.1:3000'));
    const files = await buildFileList();
};


main();
