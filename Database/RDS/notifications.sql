BEGIN;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_roles
        WHERE rolname = 'NotificationsLogin'
    ) THEN
        CREATE ROLE "NotificationsLogin" WITH
            LOGIN
            PASSWORD 'StrongPassword456!';
    END IF;
END
$$;

DO $$
BEGIN
    EXECUTE format(
        'GRANT CONNECT ON DATABASE %I TO %I',
        current_database(),
        'NotificationsLogin'
    );
END
$$;

CREATE SCHEMA IF NOT EXISTS "NotificationsSchema";
ALTER SCHEMA "NotificationsSchema" OWNER TO "NotificationsLogin";

GRANT USAGE, CREATE
    ON SCHEMA "NotificationsSchema"
    TO "NotificationsLogin";

CREATE TABLE IF NOT EXISTS "NotificationsSchema"."Notifications" (
    "NotificationId" uuid PRIMARY KEY,
    "PlayerId" uuid NOT NULL,
    "Message" text NOT NULL,
    "SentAt" timestamptz NOT NULL
);

ALTER TABLE "NotificationsSchema"."Notifications"
    OWNER TO "NotificationsLogin";

GRANT SELECT, INSERT, UPDATE, DELETE
    ON ALL TABLES IN SCHEMA "NotificationsSchema"
    TO "NotificationsLogin";

INSERT INTO "NotificationsSchema"."Notifications" (
    "NotificationId",
    "PlayerId",
    "Message",
    "SentAt"
)
SELECT
    ('00000000-0000-0000-0000-' || lpad((5000 + n)::text, 12, '0'))::uuid,
    ('00000000-0000-0000-0000-' || lpad(((n % 10) + 1)::text, 12, '0'))::uuid,
    'Test notification message #' || n,
    now() - make_interval(mins => n)
FROM generate_series(1, 20) AS n
ON CONFLICT ("NotificationId") DO NOTHING;

COMMIT;
