db = db.getSiblingDB("inventories");
db.createUser({
  user: "inventories_user",
  pwd: "inventories_pass",
  roles: [{ role: "readWrite", db: "inventories" }],
});
db.inventories.insertMany([
  {
    playerId: "player1",
    experience: 1200,
    credits: 5000,
    tanks: ["T-34", "Panther", "Sherman"],
  },
  {
    playerId: "player2",
    experience: 800,
    credits: 2000,
    tanks: ["Tiger", "KV-1"],
  },
]);
