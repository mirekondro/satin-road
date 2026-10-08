import type { ReactNode } from 'react'

/** Temporary page body until the feature is built. */
export default function Placeholder({ title, children }: { title: string; children?: ReactNode }) {
  return (
    <section className="container page">
      <h1 className="page-title">{title}</h1>
      <div className="card empty-state">{children ?? 'Coming soon…'}</div>
    </section>
  )
}
