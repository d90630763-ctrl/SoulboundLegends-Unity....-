CREATE TABLE IF NOT EXISTS commerce_products (
    product_id TEXT PRIMARY KEY,
    entitlement_id TEXT NOT NULL,
    consumable BOOLEAN NOT NULL DEFAULT TRUE,
    enabled BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS commerce_purchases (
    id BIGSERIAL PRIMARY KEY,
    player_id TEXT NOT NULL,
    product_id TEXT NOT NULL REFERENCES commerce_products(product_id),
    purchase_token TEXT NOT NULL UNIQUE,
    google_order_id TEXT,
    package_name TEXT NOT NULL,
    state TEXT NOT NULL,
    transaction_id TEXT NOT NULL UNIQUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_commerce_purchases_player
    ON commerce_purchases(player_id);

CREATE TABLE IF NOT EXISTS commerce_events (
    id BIGSERIAL PRIMARY KEY,
    purchase_token TEXT,
    event_type TEXT NOT NULL,
    payload_json TEXT NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
