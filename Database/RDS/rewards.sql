BEGIN;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_roles
        WHERE rolname = 'RewardsLogin'
    ) THEN
        CREATE ROLE "RewardsLogin" WITH
            LOGIN
            PASSWORD 'StrongPassword123!';
    END IF;
END
$$;

DO $$
BEGIN
    EXECUTE format(
        'GRANT CONNECT ON DATABASE %I TO %I',
        current_database(),
        'RewardsLogin'
    );
END
$$;

CREATE SCHEMA IF NOT EXISTS "RewardsSchema";
ALTER SCHEMA "RewardsSchema" OWNER TO "RewardsLogin";

ALTER ROLE "RewardsLogin"
    SET search_path = '"RewardsSchema"';

GRANT USAGE, CREATE
    ON SCHEMA "RewardsSchema"
    TO "RewardsLogin";

CREATE TABLE IF NOT EXISTS "RewardsSchema"."Rewards" (
    "RewardId" uuid PRIMARY KEY,
    "MinKills" integer NULL,
    "MaxDeaths" integer NULL,
    "MinDamageDealt" integer NULL,
    "MaxDamageReceived" integer NULL,
    "MinSurvived" integer NULL,
    "MinExperienceEarned" integer NULL,
    "TankId" text NULL,
    "Experience" integer NOT NULL DEFAULT 0,
    "Credits" integer NOT NULL DEFAULT 0
);

ALTER TABLE "RewardsSchema"."Rewards"
    OWNER TO "RewardsLogin";

GRANT SELECT, INSERT, UPDATE, DELETE
    ON ALL TABLES IN SCHEMA "RewardsSchema"
    TO "RewardsLogin";

INSERT INTO "RewardsSchema"."Rewards" (
    "RewardId",
    "MinKills",
    "MaxDeaths",
    "MinDamageDealt",
    "MaxDamageReceived",
    "MinSurvived",
    "MinExperienceEarned",
    "TankId",
    "Experience",
    "Credits"
)
SELECT
    ('00000000-0000-0000-0000-' || lpad((3000 + n)::text, 12, '0'))::uuid,
    CASE WHEN n % 5 = 0 THEN NULL ELSE (n % 10) + 1 END,
    CASE WHEN n % 7 = 0 THEN NULL ELSE n % 5 END,
    CASE WHEN n % 3 = 0 THEN NULL ELSE n * 100 END,
    CASE WHEN n % 4 = 0 THEN NULL ELSE n * 50 END,
    CASE WHEN n % 6 = 0 THEN NULL ELSE 1 END,
    CASE WHEN n % 2 = 0 THEN NULL ELSE n * 20 END,
    CASE WHEN n % 10 = 0 THEN 'T-34' WHEN n % 10 = 1 THEN 'Tiger I' ELSE NULL END,
    n * 50,
    n * 1000
FROM generate_series(1, 40) AS n
ON CONFLICT ("RewardId") DO NOTHING;

COMMIT;
