const path = require('path'); 
const express = require('express');
const router = express.Router();
const app = express();

process.argv.forEach(function (val, index) {
    if (val === "--port") {
        try {
            const port = parseInt(process.argv[index + 1]);
            if (port > 1023  && port < 65536) {
                app.set('port', port);
            }
        } catch (error) {
            console.log("Invalid port number");
        }
    }
});

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

const port = app.get('port');
if (port === undefined) {
    throw "Port not defined";
}

app.listen(port, () => console.log(`App listening at http://localhost:${port}`));
