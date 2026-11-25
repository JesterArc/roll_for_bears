ALTER TABLE IF EXISTS users.friends
    ADD CONSTRAINT different_users_check CHECK (user_uuid <> friend_uuid)
    NOT VALID;