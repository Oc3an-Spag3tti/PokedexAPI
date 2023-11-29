exports.success = (message, data) => {
  return {message, data}
}

const getUniqueId = (pkemons) => {
  const pokemonsIds = pkemons.map(pokemon => pokemon.id)
  const maxId = pokemonsIds.reduce((a, b) => Math.max(a, b))
  const uniqueId = maxId + 1

  return uniqueId
}
