// The client throws the response on errors (400/404/409…); .error holds ProblemDetails
// from our DomainExceptionHandler. This turns it into a readable message.
export function getErrorMessage(error: unknown): string {
  if (typeof error === 'object' && error !== null && 'error' in error) {
    const problem = (error as { error?: { title?: string | null } }).error
    if (problem?.title) return problem.title

    // Response without a body (e.g. 404 from routing, 401 from JWT) → at least show the status
    const status = (error as { status?: number }).status
    if (status) return `Request failed (HTTP ${status}).`
  }
  if (error instanceof Error) return error.message
  return 'Something went wrong.'
}
