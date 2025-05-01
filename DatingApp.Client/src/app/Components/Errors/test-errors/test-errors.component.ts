import { HttpClient } from '@angular/common/http';
import { afterNextRender, Component } from '@angular/core';
import { environment } from '../../../../environments/environment.development';

@Component({
    selector: 'app-test-errors',
    standalone: true,
    imports: [],
    templateUrl: './test-errors.component.html',
    styleUrl: './test-errors.component.css'
})

export class TestErrorsComponent {
    private baseUrl: string = environment.apiUrl;
    validationErrors: string[] = [];

    public constructor(private http: HttpClient) {}

    get404Error() {
        this.http.get(this.baseUrl + "/buggy/not-found").subscribe({
            next: (r) => console.log(r),
            error: (r) => console.log(r)
        })
    }
    
    get400Error() {
        this.http.get(this.baseUrl + "/buggy/bad-request").subscribe({
            next: (r) => console.log(r),
            error: (r) => console.log(r)
        })
    }

    get500Error() {
        this.http.get(this.baseUrl + "/buggy/server-error").subscribe({
            next: (r) => console.log(r),
            error: (r) => console.log(r)
        })
    }

    get401Error() {
        this.http.get(this.baseUrl + "/buggy/auth").subscribe({
            next: (r) => console.log(r),
            error: (r) => console.log(r)
        })
    }

    get400ValidationError() {
        this.http.post(this.baseUrl + "/account/register", {}).subscribe({
            next: (r) => console.log(r),
            error: (r) => {
                console.log(r);
                this.validationErrors = r;
            }
        })
    }
}
