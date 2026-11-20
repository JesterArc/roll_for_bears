--
-- PostgreSQL database dump
--

\restrict EBXDpXehZOsCmfEUKEIxbgfezN9j1r5WezHu4ghNVHJWa83ruKKKCV7T9nRfQ68

-- Dumped from database version 18.4 (Debian 18.4-1.pgdg13+1)
-- Dumped by pg_dump version 18.3

-- Started on 2026-11-20 11:45:50 UTC

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 6 (class 2615 OID 16515)
-- Name: gameplay; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA gameplay;


--
-- TOC entry 7 (class 2615 OID 16424)
-- Name: users; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA users;


--
-- TOC entry 871 (class 1247 OID 16561)
-- Name: game_type; Type: TYPE; Schema: gameplay; Owner: -
--

CREATE TYPE gameplay.game_type AS ENUM (
    'campaign',
    'oneshot'
);


--
-- TOC entry 874 (class 1247 OID 16573)
-- Name: player_role; Type: TYPE; Schema: gameplay; Owner: -
--

CREATE TYPE gameplay.player_role AS ENUM (
    'gamemaster',
    'player'
);


--
-- TOC entry 883 (class 1247 OID 16390)
-- Name: status; Type: TYPE; Schema: users; Owner: -
--

CREATE TYPE users.status AS ENUM (
    'active',
    'pending',
    'suspended',
    'banned'
);


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 225 (class 1259 OID 16586)
-- Name: join_requests; Type: TABLE; Schema: gameplay; Owner: -
--

CREATE TABLE gameplay.join_requests (
    connection_uuid uuid NOT NULL,
    account_uuid uuid NOT NULL,
    session_uuid uuid NOT NULL,
    player_role gameplay.player_role NOT NULL
);


--
-- TOC entry 223 (class 1259 OID 16516)
-- Name: session_details; Type: TABLE; Schema: gameplay; Owner: -
--

CREATE TABLE gameplay.session_details (
    session_uuid uuid CONSTRAINT "Session_Details_UUID_not_null" NOT NULL,
    session_name character varying(100) CONSTRAINT "Session_Details_SessionName_not_null" NOT NULL,
    tagline character varying(50),
    generating_notes boolean CONSTRAINT "Session_Details_IsGeneratingNotes_not_null" NOT NULL,
    game_system text CONSTRAINT "Session_Details_GameSystem_not_null" NOT NULL,
    game_type gameplay.game_type CONSTRAINT "Session_Details_GameType_not_null" NOT NULL,
    min_players smallint CONSTRAINT "Session_Details_MinPlayers_not_null" NOT NULL,
    max_players smallint CONSTRAINT "Session_Details_MaxPlayers_not_null" NOT NULL,
    game_language text CONSTRAINT "Session_Details_GameLanguage_not_null" NOT NULL,
    owner_uuid uuid NOT NULL,
    session_description text,
    next_session_date timestamp(0) with time zone,
    created_at timestamp(0) with time zone NOT NULL,
    last_updated_at timestamp(0) with time zone NOT NULL
);


--
-- TOC entry 224 (class 1259 OID 16532)
-- Name: session_players; Type: TABLE; Schema: gameplay; Owner: -
--

CREATE TABLE gameplay.session_players (
    connection_uuid uuid CONSTRAINT "Session_Players_ConnectionUUID_not_null" NOT NULL,
    account_uuid uuid CONSTRAINT "Session_Players_AccountUUID_not_null" NOT NULL,
    session_uuid uuid CONSTRAINT "Session_Players_SessionUUID_not_null" NOT NULL,
    player_role gameplay.player_role NOT NULL
);


--
-- TOC entry 221 (class 1259 OID 16425)
-- Name: account; Type: TABLE; Schema: users; Owner: -
--

CREATE TABLE users.account (
    account_uuid uuid CONSTRAINT "account_UUID_not_null" NOT NULL,
    hashword text CONSTRAINT "account_Password_not_null" NOT NULL,
    created_at date CONSTRAINT "account_CreatedAt_not_null" NOT NULL,
    email text CONSTRAINT "account_Email_not_null" NOT NULL,
    status_changed_at timestamp with time zone CONSTRAINT "account_StatusChangedAt_not_null" NOT NULL,
    status users.status DEFAULT 'pending'::users.status NOT NULL,
    username character varying(20)
);


--
-- TOC entry 226 (class 1259 OID 16630)
-- Name: friends; Type: TABLE; Schema: users; Owner: -
--

CREATE TABLE users.friends (
    friendship_uuid uuid NOT NULL,
    user_uuid uuid NOT NULL,
    friend_uuid uuid NOT NULL,
    friendship_mutual boolean DEFAULT false NOT NULL
);


--
-- TOC entry 222 (class 1259 OID 16443)
-- Name: refresh_token; Type: TABLE; Schema: users; Owner: -
--

CREATE TABLE users.refresh_token (
    token_uuid uuid CONSTRAINT "refresh_token_UUID_not_null" NOT NULL,
    account_uuid uuid CONSTRAINT "refresh_token_AccountId_not_null" NOT NULL,
    token_hash text CONSTRAINT "refresh_token_TokenHash_not_null" NOT NULL,
    family_uuid uuid CONSTRAINT "refresh_token_FamilyId_not_null" NOT NULL,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP CONSTRAINT "refresh_token_CreatedAt_not_null" NOT NULL,
    expires_at timestamp with time zone CONSTRAINT "refresh_token_ExpiredAt_not_null" NOT NULL,
    revoked_at timestamp with time zone,
    replaced_by_uuid uuid
);


--
-- TOC entry 3342 (class 2606 OID 16606)
-- Name: join_requests account_session_join_request_unique; Type: CONSTRAINT; Schema: gameplay; Owner: -
--

ALTER TABLE ONLY gameplay.join_requests
    ADD CONSTRAINT account_session_join_request_unique UNIQUE NULLS NOT DISTINCT (account_uuid, session_uuid);


--
-- TOC entry 3338 (class 2606 OID 16543)
-- Name: session_players account_session_session_players_unique; Type: CONSTRAINT; Schema: gameplay; Owner: -
--

ALTER TABLE ONLY gameplay.session_players
    ADD CONSTRAINT account_session_session_players_unique UNIQUE NULLS NOT DISTINCT (account_uuid, session_uuid);


--
-- TOC entry 3344 (class 2606 OID 16594)
-- Name: join_requests join_requests_pkey; Type: CONSTRAINT; Schema: gameplay; Owner: -
--

ALTER TABLE ONLY gameplay.join_requests
    ADD CONSTRAINT join_requests_pkey PRIMARY KEY (connection_uuid);


--
-- TOC entry 3336 (class 2606 OID 16531)
-- Name: session_details session_details_pkey; Type: CONSTRAINT; Schema: gameplay; Owner: -
--

ALTER TABLE ONLY gameplay.session_details
    ADD CONSTRAINT session_details_pkey PRIMARY KEY (session_uuid);


--
-- TOC entry 3340 (class 2606 OID 16541)
-- Name: session_players session_players_pkey; Type: CONSTRAINT; Schema: gameplay; Owner: -
--

ALTER TABLE ONLY gameplay.session_players
    ADD CONSTRAINT session_players_pkey PRIMARY KEY (connection_uuid);


--
-- TOC entry 3323 (class 2606 OID 16442)
-- Name: account account_email_unique; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.account
    ADD CONSTRAINT account_email_unique UNIQUE (email);


--
-- TOC entry 3325 (class 2606 OID 16436)
-- Name: account account_pkey; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.account
    ADD CONSTRAINT account_pkey PRIMARY KEY (account_uuid);


--
-- TOC entry 3327 (class 2606 OID 16440)
-- Name: account account_username_unique; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.account
    ADD CONSTRAINT account_username_unique UNIQUE (username);


--
-- TOC entry 3346 (class 2606 OID 16639)
-- Name: friends friends_pkey; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.friends
    ADD CONSTRAINT friends_pkey PRIMARY KEY (friendship_uuid);


--
-- TOC entry 3332 (class 2606 OID 16456)
-- Name: refresh_token refresh_token_pkey; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.refresh_token
    ADD CONSTRAINT refresh_token_pkey PRIMARY KEY (token_uuid);


--
-- TOC entry 3334 (class 2606 OID 16458)
-- Name: refresh_token refresh_token_token_hash_unique; Type: CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.refresh_token
    ADD CONSTRAINT refresh_token_token_hash_unique UNIQUE (token_hash);


--
-- TOC entry 3347 (class 1259 OID 16650)
-- Name: friends_user_friend_unique; Type: INDEX; Schema: users; Owner: -
--

CREATE UNIQUE INDEX friends_user_friend_unique ON users.friends USING btree (GREATEST(user_uuid, friend_uuid), LEAST(user_uuid, friend_uuid));


--
-- TOC entry 3328 (class 1259 OID 16469)
-- Name: refresh_token_account_id_idx; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX refresh_token_account_id_idx ON users.refresh_token USING btree (account_uuid);


--
-- TOC entry 3329 (class 1259 OID 16493)
-- Name: refresh_token_expires_at_idx; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX refresh_token_expires_at_idx ON users.refresh_token USING btree (expires_at);


--
-- TOC entry 3330 (class 1259 OID 16470)
-- Name: refresh_token_family_id_idx; Type: INDEX; Schema: users; Owner: -
--

CREATE INDEX refresh_token_family_id_idx ON users.refresh_token USING btree (family_uuid);


--
-- TOC entry 3353 (class 2606 OID 16600)
-- Name: join_requests account_uuid_fkey; Type: FK CONSTRAINT; Schema: gameplay; Owner: -
--

ALTER TABLE ONLY gameplay.join_requests
    ADD CONSTRAINT account_uuid_fkey FOREIGN KEY (account_uuid) REFERENCES users.account(account_uuid) MATCH FULL ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 3351 (class 2606 OID 16545)
-- Name: session_players account_uuid_fkey; Type: FK CONSTRAINT; Schema: gameplay; Owner: -
--

ALTER TABLE ONLY gameplay.session_players
    ADD CONSTRAINT account_uuid_fkey FOREIGN KEY (account_uuid) REFERENCES users.account(account_uuid) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 3350 (class 2606 OID 16555)
-- Name: session_details owner_uuid_fkey; Type: FK CONSTRAINT; Schema: gameplay; Owner: -
--

ALTER TABLE ONLY gameplay.session_details
    ADD CONSTRAINT owner_uuid_fkey FOREIGN KEY (owner_uuid) REFERENCES users.account(account_uuid) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 3354 (class 2606 OID 16595)
-- Name: join_requests session_uuid_fkey; Type: FK CONSTRAINT; Schema: gameplay; Owner: -
--

ALTER TABLE ONLY gameplay.join_requests
    ADD CONSTRAINT session_uuid_fkey FOREIGN KEY (session_uuid) REFERENCES gameplay.session_details(session_uuid) MATCH FULL ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 3352 (class 2606 OID 16550)
-- Name: session_players session_uuid_fkey; Type: FK CONSTRAINT; Schema: gameplay; Owner: -
--

ALTER TABLE ONLY gameplay.session_players
    ADD CONSTRAINT session_uuid_fkey FOREIGN KEY (session_uuid) REFERENCES gameplay.session_details(session_uuid) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 3355 (class 2606 OID 16645)
-- Name: friends account_friend_uuid_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.friends
    ADD CONSTRAINT account_friend_uuid_fkey FOREIGN KEY (friend_uuid) REFERENCES users.account(account_uuid) MATCH FULL ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 3356 (class 2606 OID 16640)
-- Name: friends account_user_uuid_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.friends
    ADD CONSTRAINT account_user_uuid_fkey FOREIGN KEY (user_uuid) REFERENCES users.account(account_uuid) MATCH FULL ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 3348 (class 2606 OID 16459)
-- Name: refresh_token refresh_token_account_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.refresh_token
    ADD CONSTRAINT refresh_token_account_fkey FOREIGN KEY (account_uuid) REFERENCES users.account(account_uuid) ON DELETE CASCADE;


--
-- TOC entry 3349 (class 2606 OID 16464)
-- Name: refresh_token refresh_token_replaced_by_fkey; Type: FK CONSTRAINT; Schema: users; Owner: -
--

ALTER TABLE ONLY users.refresh_token
    ADD CONSTRAINT refresh_token_replaced_by_fkey FOREIGN KEY (replaced_by_uuid) REFERENCES users.refresh_token(token_uuid) ON DELETE SET NULL;


-- Completed on 2026-11-20 11:45:50 UTC

--
-- PostgreSQL database dump complete
--

\unrestrict EBXDpXehZOsCmfEUKEIxbgfezN9j1r5WezHu4ghNVHJWa83ruKKKCV7T9nRfQ68

