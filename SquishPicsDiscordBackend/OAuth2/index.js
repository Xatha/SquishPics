const path = require('path'); 
const express = require('express');
const router = express.Router();
const app = express();

//app.use('',express.static(path.join(__dirname, 'public')));
app.use('',express.static(path.join(__dirname, 'public')));
app.get('/', (request, response) => {
    //return response.sendFile('index.html', { root: '.' });
    return response.sendFile(path.join(__dirname, 'index.html'));
});

app.get('/auth/discord', (request, response) => {
    //return response.sendFile('dashboard.html', { root: '.' });
    return response.sendFile(path.join(__dirname, 'dashboard.html'));
});

const port = '53134';
app.listen(port, () => console.log(`App listening at http://localhost:${port}`));
