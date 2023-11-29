const morgan = require('morgan');
const express = require('express');
const { success, getUniqueId } = require('./helper');
const favicon = require('serve-favicon');
let pokemons = require('./mock-pokemon');


const app = express();
const port = 3000;
app
  .use(favicon(__dirname + '/favicon.ico'))
  .use(morgan('dev'))

app.get('/', (req, res) => res.send("Hello, express!"));

app.get('/api/pokemons', (req, res) => {
  const message = 'La lsite des pokemons a bien ete recuperee';
  res.json(success(message, pokemons));
})

app.get('/api/pokemons/:id', (req, res) => { 
  const id = parseInt(req.params.id);
  const pokemon = pokemons.find(pokemon => pokemon.id === id);
  const message = 'Un pokemon a bien ete trouve';
  res.json(success(message, pokemon));
})

app.post('/api/pokemons', (req, res) => {
  const id = getUniqueId(pokemons);
  const pokemonCreated = { ...req.body, ...{ id: id, created: new Date() } };
  pokemon.push(pokemonCreated);
  const message = `Le pokemon ${pokemonCreated.name} a bien ete cree`;
  res.json(success(message, pokemonCreated));
})

app.listen(port, () => console.log(`Notre application est demaree sur : http://localhost:${port}`));
