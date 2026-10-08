export default function Footer() {
  return (
    <footer className="footer">
      <div className="container footer-inner">
        <span>© {new Date().getFullYear()} Satin Road</span>
        <span className="muted">School project · SEA Esbjerg · no real goods</span>
      </div>
    </footer>
  )
}
