import { Pipe, PipeTransform } from '@angular/core';
import { IEuLinkItem, IEulawLink } from '../../models/case-editor.model';
/*
 * Raise the value exponentially
 * Takes an exponent argument that defaults to 1.
 * Usage:
 *   value | exponentialStrength:exponent
 * Example:
 *   {{ 2 | exponentialStrength:10 }}
 *   formats to: 1024
*/
@Pipe({ name: 'eulinkPart' })
export class EulinkPartPipe implements PipeTransform {
    transform(value: IEulawLink): string {
        let res = '';
        if (value.itemsOther && value.itemsOther.length > 0) {
            value.itemsOther.forEach(item => {
                if (item.value && item.value !== '') {
                    res += item.label + ' ' + item.value + ', ';
                }
            });

            if (res.length > 3) {
                res = res.substr(0, res.length - 2) + ' ';
            }
        }
        let mainPartsStr = '';
        if (value.itemsBase && value.itemsBase.length > 0) {
            value.itemsBase.forEach(item => {
                if (item.value && item.value !== '') {
                    if ((item.label === 'Article') || (item.label === 'Paragraph') || (item.label === 'Alinea')
                        || (item.label === 'Point') || (item.label === 'Letter')) {

                        if (mainPartsStr === '') {
                            mainPartsStr += item.label + ' ' + item.value;
                        } else {
                            mainPartsStr += '(' + item.value + ')';
                        }
                    }
                    if ((item.label === 'Indent') || (item.label === 'Sentence')) {
                        if (mainPartsStr !== '') {
                            mainPartsStr += ', ' + item.value + ' ' + item.label.toLocaleLowerCase();
                        } else {
                            mainPartsStr += item.value + ' ' + item.label.toLocaleLowerCase();
                        }
                    }

                    if (item.label === 'Annex') {
                        if (mainPartsStr !== '') {
                            mainPartsStr = item.label + ' ' + item.value + ', ' + mainPartsStr;
                        } else {
                            mainPartsStr = item.label + ' ' + item.value + mainPartsStr;
                        }
                    }
                    if ((item.label === 'Recital')) {
                        if (mainPartsStr === '') {
                            mainPartsStr += item.label + ' ' + item.value;
                        } else {
                            mainPartsStr += ', ' + item.label + ' ' + item.value;
                        }
                    }
                }
            });
        }
        res += ' ' + mainPartsStr;

        if (res !== ' ') {
            res = ' - ' + res;
        }
        return res;
    }
}

@Pipe({ name: 'eulawhref' })
export class EulawHrefPipe implements PipeTransform {
    transform(value: IEulawLink): string {
        let res = '';
        if (value.celex) {
            res += 'https://eur-lex.europa.eu/legal-content/EN/ALL/?uri=CELEX:' + value.celex;
        }
        return res;
    }
}
