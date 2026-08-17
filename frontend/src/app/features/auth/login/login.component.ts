import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly loading = signal(false);

  readonly form = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  onSubmit(): void {
    if (this.form.invalid) {
      // Untouched controls show no error until the user has tried to submit.
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.form.disable();

    this.authService.login(this.form.getRawValue()).subscribe({
      // The token is stored by AuthService before this runs.
      next: () => this.router.navigateByUrl(this.returnUrl()),
      error: () => {
        // The error interceptor has already shown the API message.
        this.loading.set(false);
        this.form.enable();
      }
    });
  }

  /** Honours the returnUrl the auth guard attaches when it blocks a route. */
  private returnUrl(): string {
    return this.route.snapshot.queryParamMap.get('returnUrl') ?? '/contacts';
  }
}
