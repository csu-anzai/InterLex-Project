import { Pipe, PipeTransform } from '@angular/core';
import { IEucaseLink } from '../../models/case-editor.model';
/*
 * Raise the value exponentially
 * Takes an exponent argument that defaults to 1.
 * Usage:
 *   value | exponentialStrength:exponent
 * Example:
 *   {{ 2 | exponentialStrength:10 }}
 *   formats to: 1024
*/
@Pipe({ name: 'eucasePart' })
export class EucasePartPipe implements PipeTransform {
    transform(value: IEucaseLink): string {
        let res = '';
        if (value.part) {
            if (value.part !== '') {
                res += ' - Nr. ' + value.part;
            }
        }

        return res;
    }
}

@Pipe({ name: 'eucasePreliminaryRuling' })
export class EucasePreliminaryRuling implements PipeTransform {
    transform(value: IEucaseLink): string {
        let res = '';
        if (value.preliminaryRuling) {
            if (value.preliminaryRuling.code === '1') {
                res += '(PR)';
            }
        }
        return res;
    }
}

@Pipe({ name: 'eucasehref' })
export class EucaseHrefPipe implements PipeTransform {
    transform(value: IEucaseLink): string {
        let res = '';
        if (value.celex) {
            res += 'https://eur-lex.europa.eu/legal-content/EN/ALL/?uri=CELEX:' + value.celex;
        }
        return res;
    }
}
