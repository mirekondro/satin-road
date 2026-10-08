import { isRouteErrorResponse, Link, useRouteError } from 'react-router'

export default function ErrorPage() {
  const error = useRouteError()
  const is404 = !error || (isRouteErrorResponse(error) && error.status === 404)

  return (
    <section className="container page error-page">
      <p className="error-code">{is404 ? '404' : 'Error'}</p>
      <h1 className="page-title">
        {is404 ? 'This road leads nowhere.' : 'Something went wrong.'}
      </h1>
      <p className="muted">
        {is404
          ? 'The page does not exist, or the FBI shut it down.'
          : error instanceof Error
            ? error.message
            : 'Unknown error.'}
      </p>
      <Link to="/" className="btn btn-primary">Back home</Link>
    </section>
  )
}
