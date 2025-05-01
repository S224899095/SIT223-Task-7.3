import { HttpEvent, HttpHandler, HttpInterceptor, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BusyService } from '../services/busy.service';
import { delay, finalize, Observable } from 'rxjs';


@Injectable()
export class loadingInterceptor implements HttpInterceptor{

    constructor(private busyService: BusyService){ }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        this.busyService.busy();
        return next.handle(req).pipe(
            finalize(() => {
                this.busyService.idle();
            })
        );
    }
}
