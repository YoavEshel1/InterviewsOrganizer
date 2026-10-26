import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from "./structure/header/header";
import { Footer } from "./structure/footer/footer";
import { LoginArea } from "./Features/login/login-area/login-area";
import { Logo } from './Features/decorations/logo/logo';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header, Footer, LoginArea, Logo],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App { 
}
