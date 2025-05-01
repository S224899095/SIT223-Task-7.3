import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

// Mock ToastrService
class MockToastrService {
  success = jasmine.createSpy('success');
  error = jasmine.createSpy('error');
  info = jasmine.createSpy('info');
  warning = jasmine.createSpy('warning');
}

// Mock ActivatedRoute
class MockActivatedRoute {
  // You can mock specific route properties you need, like 'snapshot' or 'queryParams'
  snapshot = { params: {}, queryParams: {} };
}

describe('AppComponent', () => {
    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                AppComponent, // Import the standalone component
                HttpClientTestingModule // Required for HttpClient injection
            ],
            providers: [
                { provide: ToastrService, useClass: MockToastrService }, // Provide the mock ToastrService
                { provide: ActivatedRoute, useClass: MockActivatedRoute } // Provide the mock ActivatedRoute
            ]
        }).compileComponents();
    });

    it('should create the app', () => {
        const fixture = TestBed.createComponent(AppComponent);
        const app = fixture.componentInstance;
        expect(app).toBeTruthy();
    });

    it(`should have the 'Dating App' title`, () => {
        const fixture = TestBed.createComponent(AppComponent);
        const app = fixture.componentInstance;
        expect(app.title).toEqual('Dating App');
    });
});
