BEGIN;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_roles
        WHERE rolname = 'ClansLogin'
    ) THEN
        CREATE ROLE "ClansLogin" WITH
            LOGIN
            PASSWORD 'StrongPassword789!';
    END IF;
END
$$;

DO $$
BEGIN
    EXECUTE format(
        'GRANT CONNECT ON DATABASE %I TO %I',
        current_database(),
        'ClansLogin'
    );
END
$$;

CREATE SCHEMA IF NOT EXISTS "ClansSchema";
ALTER SCHEMA "ClansSchema" OWNER TO "ClansLogin";

ALTER ROLE "ClansLogin"
    SET search_path = '"ClansSchema"';

GRANT USAGE, CREATE
    ON SCHEMA "ClansSchema"
    TO "ClansLogin";

CREATE TABLE IF NOT EXISTS "ClansSchema"."Clans" (
    "ClanId" uuid PRIMARY KEY,
    "ClanName" text NOT NULL,
    "CreatedAt" timestamptz NOT NULL
);

CREATE TABLE IF NOT EXISTS "ClansSchema"."ClanWarResults" (
    "ClanWarId" uuid PRIMARY KEY,
    "FinishDate" timestamptz NOT NULL,
    "TotalClans" integer NOT NULL
);

CREATE TABLE IF NOT EXISTS "ClansSchema"."ClansResults" (
    "ClanResultId" uuid PRIMARY KEY,
    "ClanId" uuid NOT NULL,
    "ClanWarId" uuid NOT NULL,
    "Placement" integer NOT NULL,
    "Score" integer NOT NULL,
    CONSTRAINT "FK_ClansResults_Clans"
        FOREIGN KEY ("ClanId")
        REFERENCES "ClansSchema"."Clans" ("ClanId"),
    CONSTRAINT "FK_ClansResults_ClanWarResults"
        FOREIGN KEY ("ClanWarId")
        REFERENCES "ClansSchema"."ClanWarResults" ("ClanWarId")
);

CREATE TABLE IF NOT EXISTS "ClansSchema"."FileMetadata" (
    "FileMetadataId" uuid PRIMARY KEY,
    "FileName" varchar(255) NOT NULL,
    "FileExtension" varchar(32) NOT NULL,
    "FileSizeBytes" bigint NOT NULL,
    "CreatedAtUtc" timestamptz NOT NULL,
    "S3BucketName" varchar(128) NOT NULL,
    "S3ObjectKey" varchar(1024) NOT NULL
);

ALTER TABLE "ClansSchema"."Clans"
    OWNER TO "ClansLogin";

ALTER TABLE "ClansSchema"."ClanWarResults"
    OWNER TO "ClansLogin";

ALTER TABLE "ClansSchema"."ClansResults"
    OWNER TO "ClansLogin";

ALTER TABLE "ClansSchema"."FileMetadata"
    OWNER TO "ClansLogin";

GRANT SELECT, INSERT, UPDATE, DELETE
    ON ALL TABLES IN SCHEMA "ClansSchema"
    TO "ClansLogin";

INSERT INTO "ClansSchema"."Clans" (
    "ClanId",
    "ClanName",
    "CreatedAt"
)
SELECT
    ('00000000-0000-0000-0000-' || lpad(n::text, 12, '0'))::uuid,
    'Clan ' || n,
    now()
FROM generate_series(1, 10) AS n
ON CONFLICT ("ClanId") DO NOTHING;

COMMIT;
