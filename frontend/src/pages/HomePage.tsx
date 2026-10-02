import { Link } from 'react-router'

export default function HomePage() {
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
        {/* TODO: connect to the featured vendors API */}
        <div className="grid">
          {[1, 2, 3].map((i) => (
            <div key={i} className="card skeleton" aria-hidden />
          ))}
        </div>
      </section>
    </>
  )
}
