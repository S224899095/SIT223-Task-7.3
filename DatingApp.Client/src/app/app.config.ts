import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';
import { ErrorInterseptor } from './Interceptors/error.interceptor';
import { jwtInterceptor } from './Interceptors/jwt.interceptor';
import { loadingInterceptor } from './Interceptors/loading.interceptor';
import { BsModalService } from 'ngx-bootstrap/modal';

export const appConfig: ApplicationConfig = {
    providers: [
        provideZoneChangeDetection({ 
            eventCoalescing: true 
        }),
        provideRouter(routes),
        provideHttpClient(withInterceptorsFromDi()),
        { provide: HTTP_INTERCEPTORS, useClass: ErrorInterseptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: jwtInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: loadingInterceptor, multi: true },
        provideAnimations(),
        provideToastr({
            positionClass: 'toast-bottom-right'
        }), 
        BsModalService]
};
