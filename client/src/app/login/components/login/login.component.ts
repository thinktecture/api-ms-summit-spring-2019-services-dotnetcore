import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginModuleConfiguration } from '../../models/login-module-configuration';
import { AuthenticationService } from '../../services/authentication.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  formGroup: FormGroup;
  showError: boolean;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly authenticationService: AuthenticationService,
    private readonly authenticationConfiguration: LoginModuleConfiguration,
    private readonly router: Router,
  ) {
    this.createForm();
  }

  submit() {
    this.showError = false;

    const { username, password } = this.formGroup.value;
    this.authenticationService.login(username, password).subscribe(
      () => this.router.navigate([this.authenticationConfiguration.loginRedirectUrl]),
      () => this.showError = true,
    );
  }

  private createForm(): void {
    this.formGroup = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }
}
