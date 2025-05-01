import { HttpClient, HttpParams } from "@angular/common/http";
import { PaginatedResult } from "../Models/pagination";
import { map } from "rxjs";

export function getPaginatedResults<T>(url: string, params: HttpParams, http: HttpClient) {
    
    const paginatedResult = new PaginatedResult<T>();
    
    return http.get<T>(url, { observe: 'response', params }).pipe(
        map(response => {
            paginatedResult.result = response.body!;
            var pagination = response.headers.get('Pagination');

            if (pagination !== null) {
                paginatedResult.pagination = JSON.parse(pagination);
            }
            return paginatedResult;
        })
    );
}

export function getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    params = params.append('pageNumber', pageNumber!.toString());
    params = params.append('pageSize', pageSize!.toString());
    return params;
}