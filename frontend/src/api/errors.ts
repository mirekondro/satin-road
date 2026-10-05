
export function getErrorMessage(error: unknown): string {
    if (typeof error === 'object' && error !== null && 'error' in error) {
        const problem = (error as { error?: { title?: string | null } }).error
        if (problem?.title) return problem.title
    }
    if (error instanceof Error) return error.message
    return 'Something went wrong.'
}