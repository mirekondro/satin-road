-- Satin Road database schema

-- 1. users
CREATE TABLE users (
                       id            SERIAL PRIMARY KEY,
                       username      TEXT NOT NULL UNIQUE,
                       password_hash TEXT NOT NULL,
                       role          TEXT NOT NULL DEFAULT 'User' CHECK (role IN ('Admin', 'User')),
                       is_shut_down  BOOLEAN NOT NULL DEFAULT FALSE,
                       created_at    TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- 2. categories
CREATE TABLE categories (
                            id   SERIAL PRIMARY KEY,
                            name TEXT NOT NULL UNIQUE
);

-- 3. listings
CREATE TABLE listings (
                          id          SERIAL PRIMARY KEY,
                          vendor_id   INT NOT NULL REFERENCES users(id),
                          category_id INT NOT NULL REFERENCES categories(id),
                          title       TEXT NOT NULL,
                          description TEXT,
                          price       NUMERIC(10,2) NOT NULL CHECK (price >= 0),
                          stock       INT NOT NULL DEFAULT 0 CHECK (stock >= 0),
                          is_active   BOOLEAN NOT NULL DEFAULT TRUE,
                          created_at  TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- 4. orders
CREATE TABLE orders (
                        id               SERIAL PRIMARY KEY,
                        buyer_id         INT NOT NULL REFERENCES users(id),
                        vendor_id        INT NOT NULL REFERENCES users(id),
                        listing_id       INT NOT NULL REFERENCES listings(id),
                        quantity         INT NOT NULL CHECK (quantity > 0),
                        unit_price       NUMERIC(10,2) NOT NULL CHECK (unit_price >= 0),
                        discount_applied BOOLEAN NOT NULL DEFAULT FALSE,
                        total            NUMERIC(10,2) NOT NULL CHECK (total >= 0),
                        created_at       TIMESTAMPTZ NOT NULL DEFAULT now(),
                        CHECK (buyer_id <> vendor_id)
);

-- 5. indexes
CREATE INDEX ix_orders_buyer_vendor ON orders (buyer_id, vendor_id);  -- 20% discount
CREATE INDEX ix_orders_vendor       ON orders (vendor_id);            -- featured vendors
CREATE INDEX ix_listings_category   ON listings (category_id);
CREATE INDEX ix_listings_vendor     ON listings (vendor_id);

-- 6. seed data
INSERT INTO categories (name) VALUES
                                  ('Drugs'), ('Weapons'), ('Stolen Artifacts'), ('Counterfeits');