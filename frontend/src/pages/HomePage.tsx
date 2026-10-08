import { Link } from 'react-router'
import { useFeaturedVendors } from '../api/useFeaturedVendors.ts'

export default function HomePage() {
  const { vendors, loading, error } = useFeaturedVendors()

  return (
    <>
      <section className="hero">
        <div className="container">
          <p className="eyebrow">A discreet marketplace · est. 2026</p>
          <h1 className="hero-title">
            Smooth as <span className="text-accent">satin</span>.
          </h1>
          <p className="hero-lead">
            Buy and sell the goods nobody talks about. No questions, no traces
            (and only a 1% chance the buyer works for the FBI).
          </p>
          <div className="hero-actions">
            <Link to="/listings" className="btn btn-primary">Browse listings</Link>
            <Link to="/register" className="btn btn-ghost">Become a vendor</Link>
          </div>
        </div>
      </section>

      <section className="container page">
        <div className="section-head">
          <h2>Featured vendors</h2>
          <span className="badge">100+ sales</span>
        </div>
        {loading && (
          <div className="grid">
            {[1, 2, 3].map((i) => (
              <div key={i} className="card skeleton" aria-hidden />
            ))}
          </div>
        )}
        {error && <p className="alert alert-error">{error}</p>}

        {!loading && !error && (
          vendors.length === 0 ? (
            <div className="card empty-state">
              No vendor has passed 100 sales yet. Be the first one.
            </div>
          ) : (
            <div className="grid">
              {vendors.map((v, index) => (
                <article key={v.vendorId} className="card">
                  <p className="eyebrow">#{index + 1} featured</p>
                  <h3 className="item-title">{v.username}</h3>
                  <p className="muted">{v.ordersSold} sales</p>
                </article>
              ))}
            </div>
          )
        )}
      </section>
    </>
  )
}
