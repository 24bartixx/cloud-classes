db = db.getSiblingDB("playermatches");
db.createUser({
  user: "playermatches_user",
  pwd: "playermatches_pass",
  roles: [{ role: "readWrite", db: "playermatches" }],
});
db.playermatches.insertMany([
  {
    gameId: "game1",
    kills: 5,
    deaths: 2,
    damageDealt: 1500,
    damageReceived: 900,
    survived: true,
    experienceEarned: 300,
    credits: 1000,
  },
  {
    gameId: "game2",
    kills: 2,
    deaths: 3,
    damageDealt: 800,
    damageReceived: 1200,
    survived: false,
    experienceEarned: 150,
    credits: 500,
  },
]);
