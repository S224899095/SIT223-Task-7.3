import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../../services/account.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
    selector: 'app-register',
    standalone: true,
    imports: [FormsModule, ReactiveFormsModule],
    templateUrl: './register.component.html',
    styleUrl: './register.component.css'
})

export class RegisterComponent implements OnInit {

    @Output() cancelRegister = new EventEmitter<boolean>();
    registerForm!: FormGroup;
    validationErrors: string[] = [];

    public constructor(
        private accService: AccountService,
        private fb: FormBuilder,
        private toastr: ToastrService,
        private router: Router) {
    }

    ngOnInit(): void {
        this.initializeForm();
    }

    cancel() {
        this.cancelRegister.emit(false);
    }

    register() {
        this.accService.register(this.registerForm.value).subscribe({
            next: () => {
                this.router.navigateByUrl("/members");
            },
            error: (error: any) => {
                this.validationErrors = error;
            }
        });
    }

    initializeForm() {
        this.registerForm = this.fb.group({
            gender: ['male'],
            username: ['', Validators.required],
            knownAs: ['', Validators.required],
            dateOfBirth: ['', Validators.required],
            city: ['', Validators.required],
            country: ['', Validators.required],
            password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
            confirmPassword: ['', [Validators.required]]
        });

        // add extra validation for confirmPassword
        this.registerForm.controls['confirmPassword'].addValidators(this.matchValues('password'));
        this.registerForm.controls['password'].valueChanges.subscribe(() => {
            this.registerForm.controls['confirmPassword'].updateValueAndValidity();
        })
    }

    matchValues(matchTo: string): ValidatorFn {
        return (control: AbstractControl) => {
            var formControls = control?.parent?.controls as { [key: string]: AbstractControl };
            return control?.value === formControls[matchTo]?.value ? null : { isMatching: true };
        }
    }
}
