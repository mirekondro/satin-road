import { useState, type FormEvent } from 'react'
import { api } from '../api/client.ts'
import { getErrorMessage } from '../api/errors.ts'
import { useCategories } from '../api/useCategories.ts'
import CategoryRow from '../components/categories/CategoryRow.tsx'

export default function AdminCategoriesPage() {
  const { categories, loading, error, reload } = useCategories()
  const [newName, setNewName] = useState('')
  const [formError, setFormError] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)

  const handleCreate = async (e: FormEvent) => {
    e.preventDefault() 
    setSaving(true)
    setFormError(null)
    try {
      await api.categories.categoriesCreate({ name: newName })
      setNewName('')
      reload()
    } catch (err) {
      setFormError(getErrorMessage(err)) 
    } finally {
      setSaving(false)
    }
  }

  return (
      <section className="container page">
        <h1 className="page-title">Categories</h1>

        <form className="card inline-form" onSubmit={handleCreate}>
          <label htmlFor="new-category" className="sr-only">New category name</label>
          <input
              id="new-category"
              className="input"
              placeholder="New category, e.g. Poisons"
              value={newName}
              onChange={(e) => setNewName(e.target.value)}
              maxLength={50}
          />
          <button className="btn btn-primary" disabled={saving || !newName.trim()}>
            {saving ? 'Adding…' : 'Add'}
          </button>
        </form>
        {formError && <p className="alert alert-error">{formError}</p>}

        {loading && <p className="muted">Loading…</p>}
        {error && <p className="alert alert-error">{error}</p>}

        {!loading && !error && (
            categories.length === 0 ? (
                <div className="card empty-state">No categories yet.</div>
            ) : (
                <ul className="card item-list">
                  {categories.map((c) => (
                      <CategoryRow key={c.id} category={c} onChanged={reload} />
                  ))}
                </ul>
            )
        )}
      </section>
  )
}