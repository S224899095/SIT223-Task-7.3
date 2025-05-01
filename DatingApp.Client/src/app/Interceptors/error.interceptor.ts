import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, Observable, throwError } from 'rxjs';

@Injectable()
export class ErrorInterseptor implements HttpInterceptor {

    constructor(private router: Router, private toaster: ToastrService) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        var output = next.handle(req).pipe(
            catchError((error: HttpErrorResponse) => {
                if (error) {
                    switch (error.status) {
                        case 400:
                            // validation errors from UI
                            if (error.error.errors) {
                                const modelStateErrors = [];
                                for (const key in error.error.errors) {
                                    if (error.error.errors[key]) {
                                        modelStateErrors.push(error.error.errors[key])
                                    }
                                }
                                throw modelStateErrors.flat();
                            }
                            // object errors - coming from API
                            else if (typeof (error.error === 'object')) {
                                this.toaster.error(error.error, error.status.toString());
                            }
                            // general error when api is not reached
                            else {
                                this.toaster.error('Not a good request', error.status.toString());
                            }
                            break;
                        case 401:
                            this.toaster.error('Unauthorized', error.status.toString());
                            break;
                        case 404:
                            // Do something if you want a response
                            break;
                        case 500:
                            const navigationExtras: NavigationExtras = { state: { error: error.error } };
                            this.router.navigateByUrl('/server-error', navigationExtras);
                            break;
                        default:
                            this.toaster.error('Something went wrong...');
                            break;
                    }
                }
                throw error;
            })
        );
        return output;
    }
}

