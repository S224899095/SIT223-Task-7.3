
export interface PaginationModel {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

export class PaginatedResult<T>{
    result: T | undefined;
    pagination?: PaginationModel;
}