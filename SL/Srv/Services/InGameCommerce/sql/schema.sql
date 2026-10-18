CREATE TABLE IF NOT EXISTS commerce_transactions (
  id BIGSERIAL PRIMARY KEY,
  player_id TEXT NOT NULL,
  idempotency_key TEXT NOT NULL UNIQUE,
  transaction_type TEXT NOT NULL,
  merchant_id TEXT,
  product_id TEXT,
  currency_type TEXT,
  currency_amount BIGINT,
  status TEXT NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS auction_listings (
  id UUID PRIMARY KEY,
  seller_id TEXT NOT NULL,
  item_instance_id TEXT NOT NULL,
  starting_bid BIGINT NOT NULL,
  buyout_price BIGINT NOT NULL DEFAULT 0,
  current_bid BIGINT NOT NULL,
  highest_bidder_id TEXT,
  status TEXT NOT NULL,
  starts_at TIMESTAMPTZ NOT NULL,
  ends_at TIMESTAMPTZ NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS auction_bids (
  id BIGSERIAL PRIMARY KEY,
  auction_id UUID NOT NULL REFERENCES auction_listings(id),
  bidder_id TEXT NOT NULL,
  amount BIGINT NOT NULL,
  status TEXT NOT NULL,
  idempotency_key TEXT NOT NULL UNIQUE,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS auction_escrow (
  auction_id UUID PRIMARY KEY REFERENCES auction_listings(id),
  seller_id TEXT NOT NULL,
  item_instance_id TEXT NOT NULL,
  escrow_status TEXT NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS commerce_audit_log (
  id BIGSERIAL PRIMARY KEY,
  player_id TEXT,
  action TEXT NOT NULL,
  reference_id TEXT,
  details_json TEXT,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_auction_status_end ON auction_listings(status, ends_at);
CREATE INDEX IF NOT EXISTS idx_auction_seller ON auction_listings(seller_id);
CREATE INDEX IF NOT EXISTS idx_auction_bidder ON auction_bids(bidder_id);
